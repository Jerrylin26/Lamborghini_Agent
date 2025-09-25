from bs4 import BeautifulSoup
from selenium.webdriver.common.by import By
import time
import undetected_chromedriver as uc
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from urllib3.exceptions import NewConnectionError
from selenium.common.exceptions import WebDriverException
from selenium.webdriver.chrome.options import Options
import datetime
from dateutil.parser import parse
import pandas as pd
from selenium.common.exceptions import TimeoutException
import json
import requests
import os
import sys

class Lamborghini:

    def __init__(self, version_main=140):
        self.version_main= version_main
        # stop_date_str = pd.read_csv('./天下雜誌/天下雜誌_update.csv')['date'][0]
        # self.stopLineDate = pd.to_datetime(stop_date_str)  
        self.page = 1
        self.articles_list = []
        self.stop_scraping = False # 標記法

    # 拿來關閉阻擋的cookies,因為會使程式無法繼續
    def cookies_block(self,driver):

        try:
            cookies_button = driver.find_element(By.ID,'onetrust-accept-btn-handler')
            cookies_button.click()
            print('✅成功移除cookies阻攔')
        except:
            pass


    def start_driver(self):
        """啟動瀏覽器"""
        print("--------------- 啟動瀏覽器 --------------")
        try:        
            # 能夠通過 Cloudflare
            options = Options()
            # options.add_argument("--incognito")  # 無痕模式
            driver = uc.Chrome(version_main=self.version_main,options=options)
            driver.get(f"https://configurator.lamborghini.com/configurator/configuration/ePl9IIpV7ZNLcsMgDIav0gtkatI8lzyNE8vGYMcNd-ndCxK0i3Yy3XQXz7_4RvwChKUfBYO4bba7IzvUX_ldfu4pton9WwP8ldX1X17EsN3LkeEPMfvDS_7O-XVbJ4ckpNYnIdlzElJwSZnWrUlCMiIJ6caSMt1lSEIKTVKmOK9JiS7c-xgRIgdLoGpEV2gr2Oq5VoALgpCOPKLuI_lQYKzgBXmkd4EiSzld8YU8aiwRXa9hhKQlI79goXTjTfHMnK5h1m5aECIfOIGoUKow0RK0tmRZ1UOBcnovVor0cqAIWEX7eBkoy5tyehAzLQVdCgy6o_Rg1TQj9KZExqV4ZlnhncpZpUcPcO0blwcNhBgP5pJJWr61eVhB6Ws75zEF3S3T6YoE3eDziEJ7tc09Nw300k0TZvSeM9BIQZ98bhXwXEmGPq876_P4Qwiu2XOk22j4HikujesyRQPNrB_M718a-dm-z_b9t_Z90LQfn8fMbLE/exterior?lang=chi&country=tw")
            driver.set_page_load_timeout(15) # 不出來最多等15秒
            wait = WebDriverWait(driver,5)

        except Exception as e:
            print(f'❌瀏覽出問題: {e}')

        

        print("--------------- 登入configuration --------------")

        self.cookies_block(driver)

        simple_bar = wait.until(EC.presence_of_element_located((By.CSS_SELECTOR,'ol.ng-star-inserted')))
        color_box_href = simple_bar.find_elements(By.TAG_NAME,'li')[0].find_element(By.TAG_NAME,'a').get_attribute('href')
        driver.get(color_box_href)
        time.sleep(1)

        

        print("--------------- 登入車漆選擇頁面 --------------")

        self.cookies_block(driver)

        paint_container = wait.until(EC.presence_of_element_located((By.CSS_SELECTOR,'div.paint-container')))
        color_base = paint_container.find_elements(By.TAG_NAME,'button')[1:]

        final_list=[]
        for color in color_base[8:]:
            color_name = color.get_attribute('aria-label')
            print(color_name)
            color.click()

            
            for i in range(2):
                if (i==1):
                    
                    color_type_name = "Matt"
                    color_type = wait.until(EC.element_to_be_clickable((By.ID,'1')))
                    driver.execute_script("arguments[0].click();", color_type)
                    
                else:

                    color_type_name = "Shiny"
                    color_type = wait.until(EC.element_to_be_clickable((By.ID,'0')))
                    driver.execute_script("arguments[0].click();", color_type)
                    
                try:
                    cards = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR, 'lcc-paint-card a.part-card-container')))
                except TimeoutException:
                    print("✅確保這個顏色欄底下都沒有")
                    continue

                for card in cards:
                    card_subname = card.text
                    print(card_subname)
                    driver.execute_script("arguments[0].click();", card)
                    time.sleep(1.5)
                    try:
                        choiceNoReason = driver.find_element(By.CSS_SELECTOR,'lcc-conflict-solution.ng-star-inserted')
                        # print(choiceNoReason.get_attribute('outerHTML'))
                        print(choiceNoReason.is_displayed()) 
                        # time.sleep(3)
                        driver.execute_script("arguments[0].click();", choiceNoReason)
                        submit = wait.until(EC.element_to_be_clickable((By.CSS_SELECTOR,'a.dark-filled-button.ng-star-inserted')))
                        driver.execute_script("arguments[0].click();", submit)
                        print("✅初始化設定選擇成功")
                        time.sleep(7)

                    except:
                        pass

                    # 抓圖片
                    # 點擊圖片總欄button
                    print('.',end='')
                    img_storage = driver.find_elements(By.CSS_SELECTOR,'lcc-entertainment-menu-item')[-1]
                    driver.execute_script("arguments[0].click();", img_storage)
                    print('.',end='')
                    time.sleep(3)
                    # 要重抓 因為渲染
                    locator = (By.CSS_SELECTOR,'lcc-visual-selection-item')
                    imgs = wait.until(EC.presence_of_all_elements_located(locator))

                    # 共10張
                    img_set = set()
                    for i in range(len(imgs)):
                        # 重新抓最新的 imgs，避免 stale
                        imgs = driver.find_elements(*locator)
                        target = imgs[i]

                        # 等待第 i 個元素可點擊
                        WebDriverWait(driver, 5).until(
                            EC.element_to_be_clickable((By.CSS_SELECTOR, f'lcc-visual-selection-item:nth-of-type({i+1})'))
                        )

                        driver.execute_script("arguments[0].click();", target)
                        time.sleep(3)

                        img_box = wait.until(EC.presence_of_element_located((By.CSS_SELECTOR,'div.drag-container.ng-star-inserted')))
                        srcs = img_box.find_elements(By.TAG_NAME,'img')
                        for src in srcs:
                            src = src.get_attribute('src')
                            img_set.add(src)
                            if len(srcs) != 2:
                                print('❌不確定異常,不是兩張圖片')
                        time.sleep(1)
                        print('.',end='')
                    print(len(img_set))
                    final_list.append({'color':color_name,'color_subname':card_subname,'color_type_name':color_type_name,'img':list(img_set)})

                    with open('./Urus_SE_2.json','w',encoding='utf-8') as f:
                        json.dump(final_list,f,indent=4,ensure_ascii=False)
                
            
                
        
    def download_img(self):

        # 設定 logger
        class Logger:
            def __init__(self, filename="output.log"):
                self.terminal = sys.stdout   # 原本的 console
                self.log = open(filename, "a", encoding="utf-8")

            def write(self, message):
                self.terminal.write(message)   # 照樣印到 console
                self.log.write(message)        # 同時寫進 log
                self.log.flush()               # 立即寫入檔案

            def flush(self):
                pass  # for Python buffer 需求
        
        # 把 stdout 和 stderr 都導向 Logger
        sys.stdout = Logger("./Urus_SE.log")
        sys.stderr = sys.stdout

        try:

            df = pd.read_json("./Urus_SE.json")

            for idx,info in df.iterrows():
                
                try:
                    print(idx)
                    title = info['color_subname']
                    path_count = 1
                    path = f"./Lamborghini_pic/Urus_SE/{title}"
                    while os.path.exists(path):
                        path = f"./Lamborghini_pic/Urus_SE/{title}_({path_count})"
                        path_count += 1

                    os.makedirs(path, mode=0o777)
                    
                    imgs = info["img"]
                
                except Exception as e:
                    print(f'❌建立資料夾出問題: {e}')

                count = 0
                
                for img in imgs:
                    try:
                        
                        thumbnail = requests.get(img)
                        count += 1
                        
                        file: str = os.path.join(path,str(count))
                        with open(f'{file}.png', 'wb') as f:
                            f.write(thumbnail.content)

                    except Exception as e:
                        print(f'❌下載單張圖片出問題: {e}')

                print("✅成功抓到完整圖片")
                
                
                
            

            
        except Exception as e:
            print(f'❌下載圖片出問題: {e}')
    

if __name__ == "__main__":
    b = Lamborghini()
    b.download_img()

    # df = pd.read_json('./Lamborghini_Revuelto_info.json')
    # df1 = pd.read_json('./Lamborghini_Revuelto_info_1.json')
    # df2 = pd.read_json('./Lamborghini_Revuelto_info_2.json')

    # final = pd.concat([df,df1,df2]).reset_index(drop=True)
    # final.to_json('./Lamborghini_Revuelto_info_100.json',indent=4,force_ascii=False,orient="records")

    # with open('./Lamborghini_info_100.json', 'r', encoding='utf-8') as f:
    #     text = f.read()

    # text = text.replace("https:\/\/configuratormedia.lamborghini.com\/renderservice\/media\/fast\/", "https://configuratormedia.lamborghini.com/renderservice/media/fast/")

    # with open('./Lamborghini_info_100.json', 'w', encoding='utf-8') as f:
    #     f.write(text)

    # df = pd.read_json('./Urus_SE.json')

    # count = 1
    # for idx, info in df.iterrows():
    #     if len(info["img"])==10:
    #         print(info['color'],info["color_subname"])
    #         print(count)
    #         count +=1



